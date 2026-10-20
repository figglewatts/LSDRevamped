audio = GetEntity(this.GameObject.Name .. "Audio").DreamAudio

interacted = false
moveSpeed = 5

-- the first bird of each flock is always about
leaders = { Bird1 = true, Bird4 = true, Bird7 = true }
lateFlock = { Bird14 = true, Bird15 = true, Bird16 = true, Bird17 = true }

function start()
    local name = this.GameObject.Name
    if (lateFlock[name] and not IsDayLinear(3, 1)) or (not leaders[name] and IsDayLinear(3, 0)) then
        this.GameObject.SetActive(false)
        return
    end

    this.SetChildVisible(false)
end

function update()
    if not interacted then return end

    this.Rotate(Unity.Vector3(0, 5, 0))
    this.MoveInDirection(this.GameObject.ForwardDirection, moveSpeed)
end

function interact()
    if interacted then return end
    interacted = true
    this.GameObject.Scale = Unity.Vector3(2, 2, 2)
    this.SetChildVisible(true)
    audio.Play()
    this.LogGraphContribution(2, -7)
end