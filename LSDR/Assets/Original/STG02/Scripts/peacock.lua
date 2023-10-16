require "dreams"

player = GetEntity("__player")
distanceToPlayer = 0

function start()
    if not IsDayEven() or Random.OneIn(2) then
        this.GameObject.SetActive(false)
        return
    end
end

function intervalUpdate()
    distanceToPlayer = (player.WorldPosition - this.GameObject.WorldPosition).length()

end

function update()
    if distanceToPlayer < 0.4 and not linked then
        linked = true
        DreamSystem.SetNextTransitionDream(dreams.Happytown)
        DreamSystem.TransitionToDream()
    end
end

function interact()
    local target = player.WorldPosition
    this.LookAt(player.WorldPosition)
    this.PlayAnimation(0)
    this.LogGraphContribution(1, 4)
end