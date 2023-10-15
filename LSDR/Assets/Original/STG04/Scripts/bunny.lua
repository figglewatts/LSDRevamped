require "dreams"

player = GetEntity("__player")

interacted = false

function start()
    if not IsDayEven() then
        this.GameObject.SetActive(false)
        return
    end

    this.PlayAnimation(0)
end

function intervalUpdate()
    if not interacted then return end

    local playerDistance = (player.WorldPosition - this.GameObject.WorldPosition).length()
    if playerDistance < 0.3 then
        interacted = false
        DreamSystem.SetNextTransitionDream(dreams.MoonlightTower)
        DreamSystem.TransitionToDream()
    end
end

function interact()
    interacted = true
    DreamSystem.LogGraphContributionFromEntity(0, -2)
end