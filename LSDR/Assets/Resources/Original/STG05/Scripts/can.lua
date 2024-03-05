audio = GetEntity("CanAudio").DreamAudio

function start()
    this.SetChildVisible(false)
    
    if IsDayEven() then
        this.GameObject.SetActive(false)
        return
    end
end

function interact()
    this.LogGraphContribution(3, -1)
    this.SetChildVisible(true)
    this.Action
        .Do(|| audio.Play())
        .Then(|| this.PlayAnimation(0))
        .ThenWaitUntil(this.WaitForAnimation(0))
        .Then(|| this.StopAnimation())
        .Then(|| audio.Stop())
        .ThenFinish()
end