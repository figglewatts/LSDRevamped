require "dreams"

player = GetEntity("__player")
audio = GetEntity("BabyAudio").DreamAudio

function start()
    this.PlayAnimation(0)

    if not IsWeekDay(1) or not IsWeekDay(5) then
        this.GameObject.SetActive(false)
        return
    end
end

function interact()
    audio.Play()

    this.LogGraphContribution(-3, -4)

    this.PlayAnimation(1)

    DreamSystem.SetNextTransitionDream(dreams.ViolenceDistrict)
    DreamSystem.TransitionToDream()
end