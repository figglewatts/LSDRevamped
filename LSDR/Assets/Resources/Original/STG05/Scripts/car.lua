require "dreams"

player = GetEntity("__player")

interacted = false
linked = false
runsYou = false
running = false
slideTime = 0
moveSpeed = 1
moves = false

function start()
    -- it turns up in one of two places depending on the day
    local elsewhere = this.GameObject.Name ~= "Car"
    if not IsDayEven() or IsDayLinear(3, 1) ~= elsewhere then
        this.GameObject.SetActive(false)
        return
    end

    -- the one by the crash stays put, the other drives off (its model faces backwards)
    moves = elsewhere
    if moves then this.LookInDirection(this.Forward.negated()) end

    runsYou = Random.OneIn(5)
end

function update()
    if not interacted or slideTime > 50 then return end

    slideTime = slideTime + Unity.DeltaTime()
    if moves then this.MoveInDirection(this.Forward, moveSpeed) end

    if runsYou and not running and slideTime > 15 then
        running = true
        player.PlayerMovement.StartMovingForward()
    elseif running and slideTime > 50 then
        running = false
        player.PlayerMovement.StopMovementAndRotation()
    end
end

function intervalUpdate()
    if not interacted or linked then return end

    local playerDistance = (player.WorldPosition - this.GameObject.WorldPosition).length()
    if playerDistance < 0.75 then
        linked = true
        DreamSystem.SetNextTransitionDream(dreams.NaturalWorld)
        DreamSystem.TransitionToDream()
    end
end

function interact()
    this.LogGraphContribution(-7, 6)

    if interacted then return end
    interacted = true
    this.StopAnimation()
end