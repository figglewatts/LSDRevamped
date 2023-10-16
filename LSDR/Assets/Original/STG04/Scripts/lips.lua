require "dreams"

player = GetEntity("__player")

interacted = false
moveSpeed = 0.3

function start()
    this.PlayAnimation(0)

    if not Random.OneIn(4) then
        this.GameObject.Scale = Unity.Vector3(0.2, 0.2, 0.2)
    end

    this.SetChildVisible(false)
end

function intervalUpdate()
    if not interacted then return end

    local playerDistance = (player.WorldPosition - this.GameObject.WorldPosition).length()
    if playerDistance < 0.3 then
        interacted = false
        DreamSystem.SetNextTransitionDream(dreams.FleshTunnels)
        DreamSystem.TransitionToDream()
    end
end

function update()
    if not interacted then return end

    this.LookAtPlane(player.WorldPosition)
    this.MoveTowards(player.WorldPosition, moveSpeed)
end

function interact()
    interacted = true
    this.SetChildVisible(true)
    this.LogGraphContribution(-4, -4)
end