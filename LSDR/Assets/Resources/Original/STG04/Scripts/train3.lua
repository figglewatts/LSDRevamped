train = require "train"

player = GetEntity("__player")
audio = GetEntity(this.GameObject.Name .. "Audio").DreamAudio

moveSpeed = 2.5
completedMotion = false
interacted = false
met = false
waitTime = -1
riding = false
rideTime = 0

function start()
    if not IsDayEven() then
        this.GameObject.SetActive(false)
        return
    end

    train.generateTargetsFrom(1)
    this.SetChildVisible(false)
end

function intervalUpdate()
    if not interacted or met then return end

    local playerDistance = (player.WorldPosition - this.GameObject.WorldPosition).length()
    if playerDistance < 1 then
        met = true
        waitTime = 0
    end
end

function update()
    if riding then
        -- carried along sideways, then off somewhere
        player.WorldPosition = this.GameObject.WorldPosition + Unity.Vector3(0, 0.3, 0)
        rideTime = rideTime + Unity.DeltaTime()
        if rideTime > 5 then
            riding = false
            DreamSystem.TransitionToDream()
        end
        return
    end

    if waitTime < 0 then return end

    -- press the button as it reaches you to hop on, otherwise it takes you straight away
    waitTime = waitTime + Unity.DeltaTime()
    if Input.Interact.WasPressed then
        waitTime = -1
        riding = true
        SetCanControlPlayer(false)
        DreamSystem.SetPlayerGravity(false)
        player.WorldRotation = this.GameObject.WorldRotation + Unity.Vector3(0, -90, 0)
    elseif waitTime > 0.5 then
        waitTime = -1
        DreamSystem.TransitionToDream()
    end
end

function interact()
    interacted = true
    this.SetChildVisible(true)
    this.LogGraphContribution(-2, -6)
    audio.Play()
    this.PlayAnimation(0)
    this.Action
        .Do(function() completedMotion = train.moveTowardsTarget(this, moveSpeed) end)
        .Until(Condition.Custom(|| completedMotion))
        .Then(|| train.incrementTarget(this))
        .ThenLoop()
end

