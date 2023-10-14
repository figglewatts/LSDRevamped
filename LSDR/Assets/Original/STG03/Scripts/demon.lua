require "dreams"

audio = GetEntity(this.GameObject.Name .. "Audio").DreamAudio

function start()
    this.SetChildVisible(false)
end

function interact()
    SetCanControlPlayer(false)
    audio.Play()
    DreamSystem.LogGraphContributionFromEntity(2, -5)
    this.SetChildVisible(true)

    this.Action
        .Do(|| this.PlayAnimation(0))
        .ThenWaitUntil(this.WaitForAnimation(0))
        .Then(|| this.StopAnimation())
        .Then(|| link())
        .ThenFinish()
end

function link()
    DreamSystem.SetNextTransitionDream(dreams.Kyoto)
    DreamSystem.TransitionToDream()
end