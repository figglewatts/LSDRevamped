audio = GetEntity(this.GameObject.Name .. "Audio").DreamAudio

interacted = false

function start()
    this.SetChildVisible(false)
    
    if IsDayEven() then
        this.GameObject.SetActive(false)
        return
    end
end

function interact()
    if interacted then return end
    interacted = true

    this.SetChildVisible(true)
    this.PlayAnimation(0)
    audio.Play()
    this.LogGraphContribution(9, 0)

    this.Action
        .WaitUntil(this.WaitForAnimation(0, this.GetAnimationLengthSeconds(0) * 4))
        .Then(|| this.GameObject.SetActive(false))
        .ThenFinish()
end