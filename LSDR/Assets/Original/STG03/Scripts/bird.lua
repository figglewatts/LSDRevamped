audio = GetEntity(this.GameObject.Name .. "Audio").DreamAudio

interacted = false
moveSpeed = 0.75

function start()
    this.SetChildVisible(false)
end

function update()
    if not interacted then return end

    this.LookTowards(this.GameObject.WorldPosition + this.GameObject.RightDirection, 10)
    this.MoveInDirection(this.GameObject.ForwardDirection, moveSpeed)
end

function interact()
    interacted = true
    this.SetChildVisible(true)
    audio.Play()
    this.LogGraphContribution(2, -7)
end