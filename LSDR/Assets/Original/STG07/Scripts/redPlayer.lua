require "dreams"

player = GetEntity("__player")

interacted = false

function start()
    if not IsWeekDay(5) then
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
    this.LogGraphContribution(3, 0)
    interacted = true
end