audio = GetEntity("BellAudio").DreamAudio

function start()
    if Random.OneIn(2) then
        this.GameObject.SetActive(false)
        return
    end
end

function interact()
    DreamSystem.LogGraphContributionFromEntity(-5, 5)

    this.Action
        .Do(|| audio.Play())
        .Then(|| this.PlayAnimation(0))
        .ThenWaitUntil(this.WaitForAnimation(0))
        .ThenLoop()
end