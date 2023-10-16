require "dreams"

player = GetEntity("__player")

interacted = false
small = false
moveSpeed = 0.2

function intervalUpdate()
    if not interacted then return end
    local distanceToPlayer = (player.WorldPosition - this.GameObject.WorldPosition).length()
    if distanceToPlayer < 0.4 and not linked then
        linked = true
        DreamSystem.SetNextTransitionDream(dreams.TempleDojo)
        DreamSystem.TransitionToDream()
    elseif distanceToPlayer < 4 and not small then
        small = true
        this.GameObject.Scale = Unity.Vector3(0.25, 0.25, 0.25)
    elseif distanceToPlayer >= 4 and small then
        small = false
        this.GameObject.Scale = Unity.Vector3(1, 1, 1)
    end
    this.SnapToFloor()
end

function update()
    if not interacted then return end
    this.MoveInDirection(this.Forward, moveSpeed)
end

function interact()
    interacted = true
    this.LogGraphContribution(2, 8)
    this.PlayAnimation(0)
end