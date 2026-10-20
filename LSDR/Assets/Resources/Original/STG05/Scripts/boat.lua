audio = GetEntity(this.GameObject.Name .. "Audio").DreamAudio

moveSpeed = 0.1

function start()
    if this.GameObject.Name ~= "Boat2" and not IsDayEven() then
        this.GameObject.SetActive(false)
        return
    end
end

function update()
    this.MoveInDirection(this.Forward, moveSpeed)
end