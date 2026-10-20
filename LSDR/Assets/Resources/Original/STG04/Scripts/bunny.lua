require "dreams"

player = GetEntity("__player")

interacted = false
standTime = 0

function start()
    this.PlayAnimation(0)
    
    if not IsDayEven() then
        this.GameObject.SetActive(false)
        return
    end
end

function intervalUpdate()
    if not interacted then return end

    local playerDistance = (player.WorldPosition - this.GameObject.WorldPosition).length()
    if playerDistance < 0.3 then
        interacted = false
        DreamSystem.SetNextTransitionDream(dreams.MoonlightTower)
        DreamSystem.TransitionToDream()
    end
end

function update()
    if not interacted then return end

    -- stands for a while, steps forward, turns, then hops about at random
    standTime = standTime + Unity.DeltaTime()
    if standTime < 15 then return end

    if standTime < 16 then
        this.MoveInDirection(this.Forward, 0.6)
    elseif standTime < 17 then
        this.Rotate(Unity.Vector3(0, -15, 0))
    else
        local side = (Random.OneIn(2) and this.Right) or this.Right.negated()
        this.MoveInDirection(side, 1.25)

        local turn = (Random.OneIn(4) and 15) or -15
        this.Rotate(Unity.Vector3(0, turn, 0))
    end
end

function interact()
    interacted = true
    this.LogGraphContribution(0, -2)
end