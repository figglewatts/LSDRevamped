audio = GetEntity(this.GameObject.Name .. "Audio").DreamAudio

interacted = false
updateCount = 0
direction = 1

function start()
    this.SetUpdateIntervalSeconds(0.25)
    this.SetChildVisible(false)
end

function intervalUpdate()
    if not interacted then return end

    if updateCount > 8 then
        direction = direction * -1
        updateCount = 0
    end

    audio.Volume = audio.Volume - 0.075 * direction

    updateCount = updateCount + 1
end

function interact()
    interacted = true
    this.SetChildVisible(true)
    this.PlayAnimation(0)
    this.LogGraphContribution(-5, 2)
end