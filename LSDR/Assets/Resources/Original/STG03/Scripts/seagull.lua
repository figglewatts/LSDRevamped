player = GetEntity("__player")

interacted = false
landing = false
moveSpeed = 0.3

function start()
    if this.GameObject.Name ~= "Seagull1" and ChartZone() ~= "white" then
        this.GameObject.SetActive(false)
        return
    end

    -- now and then they come and land on you
    landing = ChartZone() == "blue"
end

function update()
    if not interacted then return end

    if landing then
        this.MoveTowards(player.WorldPosition + Unity.Vector3(0, 0.75, 0), 1)
    else
        this.MoveInDirection(this.Up, moveSpeed)
    end
end

function interact()
    this.LogGraphContribution(0, -1)

    if interacted then return end
    interacted = true
    this.GameObject.Scale = Unity.Vector3(2, 2, 2)
    this.PlayAnimation(0)
end