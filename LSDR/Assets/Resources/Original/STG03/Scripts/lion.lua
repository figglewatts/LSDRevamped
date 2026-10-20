require "dreams"

player = GetEntity("__player")
roarAudio = GetEntity(this.GameObject.Name .. "RoarAudio").DreamAudio
stepAudio = GetEntity(this.GameObject.Name .. "StepAudio").DreamAudio

moveSpeed = 0.2
chargeSpeed = 3.5
stalkSpeed = 1
interacted = false
mood = "wander"

function start()
    local wanders = this.GameObject.Name == "Lion1" or this.GameObject.Name == "Lion4"
    if wanders and ChartZone() ~= "cyan" then
        this.GameObject.SetActive(false)
        return
    end

    this.SetChildVisible(false)

    -- some lions turn on you once you've met them
    if Random.OneIn(2) then
        local moods = { "wander", "charge", "stalk" }
        mood = moods[Random.IntMax(3) + 1]
    end

    local directionChoice = Random.IntMax(4)
    if directionChoice == 0 then
        this.LookInDirection(this.Forward.negated())
    elseif directionChoice == 1 then
        this.LookInDirection(this.Right)
    elseif directionChoice == 2 then
        this.LookInDirection(this.Right.negated())
    end
end

function update()
    if not interacted then return end

    if mood == "charge" then
        this.LookAtPlane(player.WorldPosition)
        if this.GameObject.Scale.x < 6 then
            this.GameObject.Scale = this.GameObject.Scale + Unity.Vector3(2, 2, 2) * Unity.DeltaTime()
        end
        this.MoveInDirection(this.Forward, chargeSpeed)
    elseif mood == "stalk" then
        this.LookAtPlane(player.WorldPosition)
        this.MoveInDirection(this.Forward, stalkSpeed)
    else
        this.MoveInDirection(this.Forward, moveSpeed)
    end
end

function intervalUpdate()
    if not interacted then return end

    local toPlayer = player.WorldPosition - this.GameObject.WorldPosition
    local distanceToPlayer = toPlayer.length()

    if mood == "charge" then
        if distanceToPlayer < 0.25 then
            this.GameObject.SetActive(false)
        end
        return
    end

    if mood == "stalk" then
        if distanceToPlayer < 1.25 then
            interacted = false
            roarAudio.Play()
            if this.GameObject.Name == "Lion1" or this.GameObject.Name == "Lion4" then
                DreamSystem.SetNextTransitionDream(dreams.TempleDojo)
                DreamSystem.TransitionToDream()
            else
                DreamSystem.EndDream()
            end
        end
        return
    end

    -- push the player back if they get too close
    if distanceToPlayer < 1 then
        player.WorldPosition = player.WorldPosition + (toPlayer.normalise() * 0.5)
    end
end

function interact()
    if interacted then return end

    this.SetChildVisible(true)
    stepAudio.Play()
    this.PlayAnimation(0)
    this.LogGraphContribution(-3, 3)
    interacted = true

    -- the ones that stalk you hold you in place, but give up if they can't reach you
    if mood == "stalk" then
        SetCanControlPlayer(false)
        this.Action
            .WaitUntil(Condition.WaitForSeconds(20))
            .Then(giveUp)
            .ThenFinish()
    end
end

function giveUp()
    if not interacted then return end
    mood = "wander"
    SetCanControlPlayer(true)
end