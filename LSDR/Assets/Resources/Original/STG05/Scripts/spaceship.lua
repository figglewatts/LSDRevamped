audio = GetEntity("SpaceshipAudio").DreamAudio

interacted = false
moveSpeed = 0.5

function start()
    this.SetChildVisible(false)
    
    if not IsDayEven() then
        this.GameObject.SetActive(false)
        return
    end
end

function update()
    if not interacted then return end

    this.MoveInDirection(this.Forward, moveSpeed)
end

function interact()
    this.LogGraphContribution(-3, 1)
    this.PlayAnimation(0)
    this.SetChildVisible(true)
    interacted = true
    audio.Play()

    if Random.OneIn(4) then
        DreamSystem.StretchDream(2, 1)
    end
end