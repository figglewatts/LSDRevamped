require "dreams"

player = GetEntity("__player")

linked = false

function start()
    if not IsDayEven() or Random.OneIn(2) then
        this.GameObject.SetActive(false)
        return
    end

    this.PlayAnimation(0)
end

function update()
    local distanceToPlayer = (player.WorldPosition - this.GameObject.WorldPosition).length()
    if distanceToPlayer < 0.6 and not linked then
        linked = true
        DreamSystem.SetNextTransitionDream(dreams.Kyoto)
        DreamSystem.TransitionToDream()
    end
end

function interact()
    -- TODO: stretch kyoto
    DreamSystem.LogGraphContributionFromEntity(-3, -5)
end