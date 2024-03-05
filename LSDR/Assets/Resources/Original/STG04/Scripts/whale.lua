audio = GetEntity(this.GameObject.Name .. "Audio").DreamAudio

function start()
    this.SetChildVisible(false)
    
    if IsDayEven() then
        this.GameObject.SetActive(false)
        return
    end
end

function interact()
    this.SetChildVisible(true)
    this.PlayAnimation(0)
    audio.Play()
    this.LogGraphContribution(9, 0)
end