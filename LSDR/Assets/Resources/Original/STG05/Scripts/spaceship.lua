audio = GetEntity("SpaceshipAudio").DreamAudio

interacted = false
moveSpeed = 1.25
flightTime = 0
stretches = false

function start()
    this.SetChildVisible(false)
    
    if not IsDayEven() then
        this.GameObject.SetActive(false)
        return
    end

    if Random.OneIn(2) then
        this.GameObject.WorldPosition = this.GameObject.WorldPosition + Unity.Vector3(0, 1, 0)
    end
    stretches = Random.OneIn(10)
end

function update()
    if not interacted then return end

    this.MoveInDirection(this.Forward, moveSpeed)

    flightTime = flightTime + Unity.DeltaTime()
    if stretches and flightTime > 15 then
        stretches = false
        DreamSystem.StretchDream(2, 1)
    end
end

function interact()
    this.LogGraphContribution(-3, 1)
    if interacted then return end
    this.PlayAnimation(0)
    this.SetChildVisible(true)
    interacted = true
    audio.Play()
end