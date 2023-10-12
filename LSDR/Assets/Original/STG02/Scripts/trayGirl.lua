require "dreams"

player = GetEntity("__player")

interacted = false
distanceToPlayer = 0
moveSpeed = 1.4

function start()
    if IsDayEven() or Random.OneIn(2) then
        this.GameObject.SetActive(false)
        return
    end

    this.SetChildVisible(false)
end

function intervalUpdate()
    if not interacted then return end
    distanceToPlayer = (player.WorldPosition - this.GameObject.WorldPosition).length()
end

function update()
    if not interacted then return end

    if distanceToPlayer < 0.3 and not linked then
        linked = true
        DreamSystem.SetNextTransitionDream(dreams.PitAndTemple)
        DreamSystem.TransitionToDream()
    end

    this.MoveInDirection(this.Forward, moveSpeed)
end

function interact()
    this.SetChildVisible(true)
    this.PlayAnimation(0)
    interacted = true
    DreamSystem.LogGraphContributionFromEntity(2, 3)
end