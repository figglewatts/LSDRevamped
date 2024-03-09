player = GetEntity("__player")
playerCamera = player.PlayerMovement.Camera
videoClip = GetEntity("GearsVideoClip").VideoClip

interacted = false
playedVideo = false

function start()
    if IsDayEven() then
        this.GameObject.SetActive(false)
        return
    end
end

function intervalUpdate()
    if not interacted then return end

    local playerDistance = (player.WorldPosition - this.GameObject.WorldPosition).length()
    if playerDistance > 5 then
        return
    end

    local playerAligned = (this.GameObject.WorldPosition - player.WorldPosition).normalise().dot(player.ForwardDirection)
    if playerAligned < 0.5 then
        return
    end

    if playerCamera.LocalRotation.x > 270 and not playedVideo then
        playedVideo = true
        videoClip.Play(Unity.ColorRGB(1, 0, 0))
    end
end

function interact()
    this.LogGraphContribution(-6, 7)
    interacted = true
end