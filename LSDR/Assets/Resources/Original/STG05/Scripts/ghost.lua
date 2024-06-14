require "dreams"

audio = GetEntity(this.GameObject.Name .. "Audio").DreamAudio
videoClip = GetEntity("FlyingVideoClip").VideoClip
player = GetEntity("__player")

interacted = false
moveSpeed = 0.05

function start()
    if Random.OneIn(3) then
        this.GameObject.Scale = Unity.Vector3(4, 4, 4)
    end

    this.SetChildVisible(false)
    
    if IsDayEven() then
        this.GameObject.SetActive(false)
        return
    end
end

function intervalUpdate()
    if not interacted then return end

    local playerDistance = (player.WorldPosition - this.GameObject.WorldPosition).length()
    if playerDistance < 0.3 then
        interacted = false
        DreamSystem.SetNextTransitionDream(dreams.LongHallway)
        DreamSystem.TransitionToDream()
    end

    this.SnapToFloor(0.3)
end

function update()
    if not interacted then return end

    this.LookAtPlane(player.WorldPosition)
    this.MoveTowards(player.WorldPosition, moveSpeed)
end

function interact()
    interacted = true
    this.SetChildVisible(true)
    audio.Play()
    this.LogGraphContribution(-4, -5)

    if Random.OneIn(2) then
        DreamSystem.StretchDream(2, 1)
    end

    if Random.OneIn(1) then
        videoClip.Play(Unity.ColorRGB(1, 0, 0))
    end
end