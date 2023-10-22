require "dreams"

audio = GetEntity("CorpseKickerAudio").DreamAudio
player = GetEntity("__player")

interacted = false

function start()
    if not IsDayEven() then
        this.GameObject.SetActive(false)
        return
    end
end

function intervalUpdate()
    if not interacted then return end

    local playerDistance = (player.WorldPosition - this.GameObject.WorldPosition).length()
    if playerDistance < 0.3 then
        interacted = false
        DreamSystem.SetNextTransitionDream(dreams.ViolenceDistrict)
        DreamSystem.TransitionToDream()
    end
end

function interact()
    interacted = true
    this.LogGraphContribution(5, -1)
end