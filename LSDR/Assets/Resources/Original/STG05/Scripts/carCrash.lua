hornAudio = GetEntity("CarHornAudio").DreamAudio
splashAudio = GetEntity("CarSplashAudio").DreamAudio

function start()
    if not IsDayEven() then
        this.GameObject.SetActive(false)
        return
    end
end

function interact()
    this.LogGraphContribution(-5, 5)
    this.Action
        .Do(|| hornAudio.Play())
        .Then(|| this.PlayAnimation(0))
        .ThenWaitUntil(Condition.WaitForSeconds(3))
        .Then(|| hornAudio.Stop())
        .ThenWaitUntil(Condition.WaitForSeconds(1))
        .Then(|| splashAudio.Play())
        .ThenWaitUntil(Condition.WaitForSeconds(1.8))
        .Then(|| this.StopAnimation())
        .ThenFinish()
end