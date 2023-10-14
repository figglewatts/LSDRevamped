audio = GetEntity(this.GameObject.Name .. "Audio").DreamAudio

moveSpeed = 2.4
interacted = false

function start()
    if DreamSystem.DayNumber != 1 then
        if Random.OneIn(2) then
            this.GameObject.SetActive(false)
            return
        end
    end
    this.SetChildVisible(false)
end

function intervalUpdate()
    if not interacted then return end

    this.SnapToFloor()
end

function update()
    if not interacted then return end

    this.MoveInDirection(this.Forward, moveSpeed)
end

function interact()
    this.SetChildVisible(true)
    DreamSystem.LogGraphContributionFromEntity(-3, 1)
    audio.Play()
    this.PlayAnimation(0)
    interacted = true
end