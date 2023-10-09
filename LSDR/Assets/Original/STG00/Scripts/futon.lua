require "dreams"

audio = GetEntity("FutonAudio").DreamAudio
player = GetEntity("__player")

linked = false

function start()
    if IsDayEven() then
        this.GameObject.SetActive(false)
        return
    end
end

function update()
    if linked then return end

    local playerDist = (player.WorldPosition - this.GameObject.WorldPosition).length()
    if playerDist < 0.4 then
        linked = true
        DreamSystem.SetNextTransitionDream(dreams.MonumentPark)
        DreamSystem.TransitionToDream()
    end
end

function interact()
    audio.Play()
    this.PlayAnimation(0)
    DreamSystem.LogGraphContributionFromEntity(-4, 4)
end