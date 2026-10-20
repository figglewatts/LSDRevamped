player = GetEntity("__player")
flying = this.GameObject.Name != "HouseAngel"
interacted = false
drifting = false
flightTime = 0
distanceToPlayer = 0

function start()
    if IsDayEven() or (this.GameObject.Name == "HouseAngel" and not IsDayLinear(3, 1)) then
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
    if not interacted or not flying then return end

    if distanceToPlayer < 1 then drifting = true end

    if drifting then
        -- once you've met it, it flutters up and off to the side
        this.MoveInDirection(Unity.Vector3(0, 1, 0), 0.6)
        this.MoveInDirection(this.Right.negated(), 0.3)
        return
    end

    -- swoops in, getting faster, then gives up and flies off
    flightTime = flightTime + Unity.DeltaTime()
    if flightTime < 13 then
        this.MoveInDirection(this.Forward, 0.6 + flightTime * 0.2)
    else
        this.MoveInDirection(this.Forward.negated(), 2.5)
    end
end

function interact()
    this.LogGraphContribution(5, 1)
    this.SetChildVisible(true)
    interacted = true
end