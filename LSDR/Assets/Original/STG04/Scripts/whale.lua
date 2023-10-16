audio = GetEntity(this.GameObject.Name .. "Audio").DreamAudio

function start()
    if IsDayEven() then
        this.GameObject.SetActive(false)
        return
    end

    this.SetChildVisible(false)
end

function interact()
    this.SetChildVisible(true)
    this.PlayAnimation(0)
    audio.Play()
    this.LogGraphContribution(9, 0)
end