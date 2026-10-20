require "dreams"

player = GetEntity("__player")

interacted = false
mood = "vanish"
approachSpeed = 0.4

function start()
    if not IsDayEven() then
        this.GameObject.SetActive(false)
        return
    end

    if Random.OneIn(3) then
        mood = "leap"
    elseif Random.OneIn(2) then
        mood = "approach"
    end
end

function intervalUpdate()
    if not interacted or mood ~= "vanish" then return end

    local playerDistance = (player.WorldPosition - this.GameObject.WorldPosition).length()
    if playerDistance < 0.25 then
        interacted = false
        if not Random.OneIn(3) then
            this.GameObject.SetActive(false)
        end
    end
end

function update()
    if not interacted or mood ~= "approach" then return end

    this.LookAtPlane(player.WorldPosition)
end

function interact()
    if interacted then return end
    interacted = true
    this.LogGraphContribution(4, -5)

    if mood == "leap" then
        this.GameObject.WorldPosition = this.GameObject.WorldPosition + this.Forward * 10
        return
    end

    if mood == "vanish" then
        this.StopAnimation()
        return
    end

    this.Action
        .WaitUntil(Condition.WaitForSeconds(1))
        .Then(|| SetCanControlPlayer(false))
        .Then(|| this.MoveInDirection(this.Forward, approachSpeed))
        .Until(Condition.WaitForSeconds(1))
        .Then(playerTransition)
        .ThenFinish()
end

function playerTransition()
    DreamSystem.SetNextTransitionDream(dreams.Happytown)
    DreamSystem.SetNextTransitionColor(Colors.Red)
    DreamSystem.TransitionToDream()
end