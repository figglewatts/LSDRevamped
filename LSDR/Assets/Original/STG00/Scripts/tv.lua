require "dreams"

player = GetEntity("__player")
audio = GetEntity("TVAudio").DreamAudio

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
    if playerDist < 0.5 and not linked then
        linked = true
        DreamSystem.SetNextTransitionDream(dreams.FleshTunnels)
        DreamSystem.TransitionToDream()
    end
end

function interact()
    DreamSystem.LogGraphContributionFromEntity(0, 5)
    audio.Play()
end