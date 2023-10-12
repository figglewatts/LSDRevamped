require "dreams"

moveSpeed = 0.2
linked = false
distanceToPlayer = 0

player = GetEntity("__player")

function start()
    this.PlayAnimation(0)
end

function intervalUpdate()
    distanceToPlayer = (player.WorldPosition - this.GameObject.WorldPosition).length()
end

function update()
    this.MoveInDirection(this.GameObject.ForwardDirection.negated(), moveSpeed)

    if distanceToPlayer < 0.6 and not linked then
        linked = true
        DreamSystem.SetNextTransitionDream(dreams.PitAndTemple)
        DreamSystem.TransitionToDream()
    end
end