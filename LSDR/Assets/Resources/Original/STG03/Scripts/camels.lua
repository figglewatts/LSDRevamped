player = GetEntity("__player")
interacted = false
toPlayer = nil
lastPlayerPos = nil
totalDistance = 0

function start()
    this.SetChildVisible(false)

    local size = 2
    if Random.OneIn(2) then size = 6 end
    this.GameObject.Scale = Unity.Vector3(size, size, size)
end

function intervalUpdate()
    if not interacted then return end

    local playerPos = player.WorldPosition
    local diff = playerPos - lastPlayerPos
    local projected = diff.project(toPlayer)
    
    -- make sure we're not moving ourselves backwards
    local projectedDotToPlayer = projected.dot(toPlayer)
    if projectedDotToPlayer < 0 then
        this.GameObject.WorldPosition = this.GameObject.WorldPosition + projected
        totalDistance = totalDistance + projected.length()
    end

    -- when we've moved too far, remove ourselves
    if totalDistance > 20 then
        this.GameObject.SetActive(false)
        return
    end
    
    lastPlayerPos = playerPos
end

function interact()
    interacted = true
    this.SetChildVisible(true)
    toPlayer = (player.WorldPosition - this.GameObject.WorldPosition).normalise()
    this.LookInDirection(toPlayer)
    lastPlayerPos = player.WorldPosition
    this.LogGraphContribution(0, 7)
end