videoClip = GetEntity("FerrisWheelVideo").VideoClip
player = GetEntity("__player")

waitTime = -1
lifting = false
liftTime = 0
liftSpeed = 1

function start()
    this.PlayAnimation(0)
    
    if IsDayEven() then
        this.GameObject.SetActive(false)
        return
    end
end

function update()
    if lifting then
        liftTime = liftTime + Unity.DeltaTime()
        player.WorldPosition = player.WorldPosition + Unity.Vector3(0, liftSpeed * Unity.DeltaTime(), 0)
        if liftTime > 5 then
            lifting = false
            DreamSystem.EndDream()
        end
        return
    end

    if waitTime < 0 then return end

    waitTime = waitTime + Unity.DeltaTime()
    if Input.Interact.WasPressed then
        waitTime = -1
        lifting = true
        SetCanControlPlayer(false)
        DreamSystem.SetPlayerGravity(false)
    elseif waitTime > 1.5 then
        waitTime = -1
        videoClip.Play(Unity.ColorRGB(1, 0, 0))
    end
end

function interact()
    this.LogGraphContribution(9, 9)
    waitTime = 0
end