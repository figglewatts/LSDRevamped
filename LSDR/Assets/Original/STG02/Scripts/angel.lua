require "dreams"

player = GetEntity("__player")
flying = this.GameObject.Name != "HouseAngel"
interacted = false
distanceToPlayer = 0
moveSpeed = 0.5

function start()
    if IsDayEven() or Random.OneIn(2) then
        this.GameObject.SetActive(false)
        return
    end

    this.SetChildVisible(false)
end

function intervalUpdate()
    local toPlayer = player.WorldPosition - this.GameObject.WorldPosition
    distanceToPlayer = toPlayer.length()
    local target = player.WorldPosition
    target.y = this.GameObject.WorldPosition.y
    this.LookAt(target)
end

function update()
    if not interacted then return end

    if distanceToPlayer < 0.3 and not linked then
        linked = true
        DreamSystem.SetNextTransitionDream(dreams.Happytown)
        DreamSystem.TransitionToDream()
    end

    if not flying then return end

    this.MoveTowards(player.WorldPosition, moveSpeed)
end

function interact()
    DreamSystem.LogGraphContributionFromEntity(5, 1)
    this.SetChildVisible(true)
    interacted = true
end