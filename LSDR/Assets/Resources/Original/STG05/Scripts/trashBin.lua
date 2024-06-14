videoClip = GetEntity("FlyingVideoClip").VideoClip
player = GetEntity("__player")
playerCamera = player.PlayerMovement.Camera

interacted = false

function start()
    if Random.OneIn(2) then
        --this.GameObject.SetActive(false)
        --return
    end
end

function intervalUpdate()
    local playerDistance = (player.WorldPosition - this.GameObject.WorldPosition).length()
    if playerDistance > 2 then
        return
    end

    local playerAligned = (this.GameObject.WorldPosition - player.WorldPosition).normalise().dot(player.ForwardDirection)
    
    if playerAligned < 0.5 then
        return
    end

    if playerCamera.LocalRotation.x > 0 and playerCamera.LocalRotation.x < 180 and not interacted then
        interacted = true
        this.Action
            .Do(|| this.PlayAnimation(0))
            .ThenWaitUntil(this.WaitForAnimation(0))
            .Then(|| this.StopAnimation())
            .ThenFinish()

        if Random.OneIn(2) then
            videoClip.Play(Unity.ColorRGB(0, 0, 0))
        end
    end
end

function interact()
    this.LogGraphContribution(0, -7)
end