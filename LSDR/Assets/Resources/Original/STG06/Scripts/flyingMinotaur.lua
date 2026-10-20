require "dreams"

player = GetEntity("__player")
audio = GetEntity(this.GameObject.Name .. "Audio").DreamAudio

interacted = false
met = false
flightTime = 0
circleSpeed = 2.5
approachSpeed = 0.6

function start()
    this.SetChildVisible(false)
    
    if IsDayEven() then
        this.GameObject.SetActive(false)
        return
    end
end

function intervalUpdate()
    if not interacted or met then return end

    local playerDistance = (player.WorldPosition - this.GameObject.WorldPosition).length()
    if playerDistance < 3.5 then
        met = true
        SetCanControlPlayer(false)
        this.Action
            .Do(pushPlayer)
            .Until(Condition.WaitForSeconds(1))
            .Then(link)
            .ThenFinish()
    end
end

function update()
    if not interacted or met then return end

    flightTime = flightTime + Unity.DeltaTime()
    if flightTime < 40 then
        this.Rotate(Unity.Vector3(0, -2, 0))
        this.MoveInDirection(this.Forward, circleSpeed)
        return
    end

    -- turns on you, small at first and swelling to full size
    this.LookAtPlane(player.WorldPosition)
    this.MoveInDirection(this.Forward, approachSpeed)
    local size = math.min(1, 0.125 + (flightTime - 40) / 56 * 0.875)
    this.GameObject.Scale = Unity.Vector3(size, size, size)
end

function interact()
    this.LogGraphContribution(-5, 0)

    if interacted then return end
    this.SetChildVisible(true)
    audio.Play()
    interacted = true
    this.GameObject.WorldPosition = this.GameObject.WorldPosition + Unity.Vector3(0, 0.25, 0)
end

function pushPlayer()
    player.WorldPosition = player.WorldPosition + this.Forward * (2.5 * Unity.DeltaTime())
end

function link()
    DreamSystem.SetNextTransitionDream(dreams.Void)
    DreamSystem.TransitionToDream()
end