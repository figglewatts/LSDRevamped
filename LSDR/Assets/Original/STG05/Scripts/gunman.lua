require "dreams"

victim = GetEntity("GunmanVictim").AnimatedObject
audio = GetEntity(this.GameObject.Name .. "Audio").DreamAudio
player = GetEntity("__player")

interacted = false
moveSpeed = 0.05
rotatingPlayer = false

function start()
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
        DreamSystem.SetNextTransitionDream(dreams.ClockworkMachines)
        DreamSystem.TransitionToDream()
    end
end

function update()
    if not interacted then return end

    this.LookAtPlane(player.WorldPosition)
    this.MoveTowards(player.WorldPosition, moveSpeed)

    if not rotatingPlayer then return end
    player.LocalRotation = player.LocalRotation + Unity.Vector3(0, 0, 0.05)
end

function interact()
    audio.Play()
    this.LogGraphContribution(3, 0)

    if this.GameObject.Name == "GunmanWithVictim" then
        -- shoot victim then player
        this.Action
            .Do(|| victim.Play(0))
            .Then(|| this.LogGraphContribution(0, -8))
            .ThenWaitUntil(Condition.WaitForSeconds(0.75))
            .Then(|| victim.Stop())
            .ThenWaitUntil(Condition.WaitForSeconds(4))
            .Then(function() interacted = true end)
            .ThenWaitUntil(Condition.WaitForSeconds(0.5))
            .Then(|| SetCanControlPlayer(false))
            .Then(|| player.LookAtPlane(this.GameObject.WorldPosition))
            .Then(|| DreamSystem.EndDream())
            .Then(function() rotatingPlayer = true end)
            .ThenFinish()
    else
        -- shoot only player
        interacted = true
        this.Action
            .WaitUntil(Condition.WaitForSeconds(0.5))
            .Then(|| SetCanControlPlayer(false))
            .Then(|| player.LookAtPlane(this.GameObject.WorldPosition))
            .Then(|| DreamSystem.EndDream())
            .Then(function() rotatingPlayer = true end)
            .ThenFinish()
    end
end