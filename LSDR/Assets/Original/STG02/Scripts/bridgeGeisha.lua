require "dreams"

player = GetEntity("__player")
distanceToPlayer = 0

function start()
    if not Random.OneIn(3) then
        this.GameObject.SetActive(false)
        return
    end
end

function intervalUpdate()
    distanceToPlayer = (player.WorldPosition - this.GameObject.WorldPosition).length()
end

function update()
    if distanceToPlayer < 0.3 and not linked then
        linked = true
        DreamSystem.SetNextTransitionDream(dreams.Void)
        DreamSystem.TransitionToDream()
    end
end

function interact()
    DreamSystem.LogGraphContributionFromEntity(-5, 6)
end