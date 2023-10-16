require "dreams"

player = GetEntity("__player")

interacted = false

function start()
    if IsDayEven() then
        this.GameObject.SetActive(false)
        return
    end
end

function intervalUpdate()
    if not interacted then return end

    local playerDistance = (player.WorldPosition - this.GameObject.WorldPosition).length()
    if playerDistance < 0.4 then
        interacted = false
        DreamSystem.SetNextTransitionDream(dreams.LongHallway)
        DreamSystem.TransitionToDream()
    end
end

function interact()
    this.LogGraphContribution(0, 3)
    interacted = true
end