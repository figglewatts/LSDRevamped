require "dreams"

player = GetEntity("__player")

interacted = false
small = false
moveSpeed = 0.3

function start()
    -- a couple of little ones turn up in the red part of the chart
    if this.GameObject.Name ~= "Tortoise1" and ChartZone() ~= "red" then
        this.GameObject.SetActive(false)
    end
end

function intervalUpdate()
    if not interacted then return end
    local distanceToPlayer = (player.WorldPosition - this.GameObject.WorldPosition).length()
    if distanceToPlayer < 0.4 and not linked then
        linked = true
        DreamSystem.SetNextTransitionDream(dreams.TempleDojo)
        DreamSystem.TransitionToDream()
    elseif distanceToPlayer < 2 and not small then
        -- once you've met it, it stays small
        small = true
        this.GameObject.Scale = Unity.Vector3(0.25, 0.25, 0.25)
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