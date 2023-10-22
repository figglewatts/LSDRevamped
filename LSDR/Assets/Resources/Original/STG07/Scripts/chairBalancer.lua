require "dreams"

player = GetEntity("__player")
audio = GetEntity("ChairBalancerAudio").DreamAudio

interacted = false

function start()
    if not IsWeekDay(1) and not IsWeekDay(5) then
        this.GameObject.SetActive(false)
        return
    end
end

function intervalUpdate()
    if not interacted then return end

    local playerDistance = (player.WorldPosition - this.GameObject.WorldPosition).length()
    if playerDistance < 0.4 then
        interacted = false
        DreamSystem.SetNextTransitionDream(dreams.TempleDojo)
        DreamSystem.TransitionToDream()
    end
end

function interact()
    this.LogGraphContribution(-4, -1)
    interacted = true
    audio.Play()
end