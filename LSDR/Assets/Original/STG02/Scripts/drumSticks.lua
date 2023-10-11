audio = GetEntity("DrumStickAudio").DreamAudio

function start()
    if IsDayEven() or Random.OneIn(2) then
        this.GameObject.SetActive(false)
        return
    end
end

function interact()
    DreamSystem.LogGraphContributionFromEntity(-2, -1)

    this.Action
        .Do(|| audio.Play())
        .Then(|| this.PlayAnimation(0))
        .ThenWaitUntil(this.WaitForAnimation(0))
        .ThenLoop()
end