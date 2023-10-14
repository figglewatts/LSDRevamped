require "dreams"

player = GetEntity("__player")
target = GetEntity(this.GameObject.Name .. "Target")
startPoint = this.GameObject.WorldPosition
interacted = false
distanceToPlayer = 0
linked = false
moveSpeed = 0.3

function intervalUpdate()
    if not interacted then return end
    distanceToPlayer = (player.WorldPosition - this.GameObject.WorldPosition).length()
    if distanceToPlayer < 0.4 and not linked then
        linked = true
        DreamSystem.SetNextTransitionDream(dreams.MonumentPark)
        DreamSystem.TransitionToDream()
    end
end

function interact()
    interacted = true
    DreamSystem.LogGraphContributionFromEntity(-8, 4)
    this.Action
        .Do(|| this.LookAt(target.WorldPosition))
        .Then(|| this.MoveTowards(target.WorldPosition, moveSpeed))
        .Until(Condition.WaitForLinearMove(this.GameObject, target.WorldPosition))
        .Then(|| this.LookAt(startPoint))
        .Then(|| this.MoveTowards(startPoint, moveSpeed))
        .Until(Condition.WaitForLinearMove(this.GameObject, startPoint))
        .ThenLoop()
end
