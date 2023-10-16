require "dreams"

player = GetEntity("__player")

linked = false
distanceToPlayer = 0

function start()
    if not IsDayEven() or Random.OneIn(2) then
        this.GameObject.SetActive(false)
        return
    end

    this.PlayAnimation(0)
end

function intervalUpdate()
    distanceToPlayer = (player.WorldPosition - this.GameObject.WorldPosition).length()
end

function update()
    if distanceToPlayer < 0.6 and not linked then
        linked = true
        DreamSystem.SetNextTransitionDream(dreams.Kyoto)
        DreamSystem.TransitionToDream()
    end
end

function interact()
    -- TODO: stretch kyoto
    this.LogGraphContribution(-3, -5)
end