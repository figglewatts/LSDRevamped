audio = GetEntity(this.GameObject.Name .. "Audio").DreamAudio

moveSpeed = 0.5

function start()
end

function update()
    this.MoveInDirection(this.Forward, moveSpeed)
end