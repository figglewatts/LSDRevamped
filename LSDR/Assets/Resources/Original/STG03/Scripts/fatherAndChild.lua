require "dreams"

player = GetEntity("__player")

interacted = false
linked = false

function start()
    if this.GameObject.Name ~= "FatherAndChild" and ChartZone() ~= "yellow" then
        this.GameObject.SetActive(false)
    end
end

function intervalUpdate()
    if not interacted or linked then return end

    local distanceToPlayer = (player.WorldPosition - this.GameObject.WorldPosition).length()
    if distanceToPlayer < 0.3 then
        linked = true
        DreamSystem.SetNextTransitionDream(dreams.LongHallway)
        DreamSystem.TransitionToDream()
    end
end

function interact()
    this.LogGraphContribution(7, 1)

    if interacted then return end
    interacted = true
    this.LookAtPlane(player.WorldPosition)
end