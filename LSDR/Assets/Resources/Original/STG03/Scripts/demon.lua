require "dreams"

audio = GetEntity(this.GameObject.Name .. "Audio").DreamAudio

function start()
    if this.GameObject.Name == "BridgeDemon" and not IsDayLinear(3, 2) then
        this.GameObject.SetActive(false)
        return
    end

    this.SetChildVisible(false)
end

function interact()
    SetCanControlPlayer(false)
    audio.Play()
    this.LogGraphContribution(2, -5)
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