require "dreams"

player = GetEntity("__player")
audio = GetEntity(this.GameObject.Name .. "Audio").DreamAudio
moveSpeed = 0.25
distanceToPlayer = 0
walkTime = 0
route = nil
caught = false

function start()
    this.PlayAnimation(0)
    audio.Play()
    
    -- the other geishas only turn up on some odd days, and don't open the teleports
    if this.GameObject.Name ~= "WalkingGeisha1" then
        if IsDayEven() or Random.OneIn(2) then
            this.GameObject.SetActive(false)
        end
        return
    end

    if not IsDayEven() then
        this.GameObject.SetActive(false)
        return
    end

    DreamSystem.ArmTeleports()
end

function intervalUpdate()
    distanceToPlayer = (player.WorldPosition - this.GameObject.WorldPosition).length()
    this.SnapToFloor()
end

function update()
    if caught then
        -- she turns to you at half size and comes to meet you
        this.LookAtPlane(player.WorldPosition)
        this.MoveTowards(player.WorldPosition, 1.2)
        if distanceToPlayer < 0.5 and not linked then
            linked = true
            DreamSystem.TransitionToDream()
        end
        return
    end

    if distanceToPlayer < 0.3 and not linked then
        linked = true
        DreamSystem.SetNextTransitionDream(dreams.TempleDojo)
        DreamSystem.TransitionToDream()
    end
    
    this.MoveInDirection(this.GameObject.ForwardDirection.negated(), moveSpeed)

    if this.GameObject.Name == "WalkingGeisha1" then
        followRoute()
    end
end

function followRoute()
    local before = walkTime
    walkTime = walkTime + Unity.DeltaTime()
    local function at(seconds) return before < seconds and walkTime >= seconds end

    -- partway along she picks one of two routes
    if at(80) then
        if Random.OneIn(2) then
            route = "long"
            turn(90)
        else
            route = "short"
            turn(-90)
        end
    end

    if route == "long" then
        if at(135) or at(180) then turn(-90) end
        if at(160) then turn(90) end

        -- near the end of the long route, pressing the button catches her attention
        if walkTime > 170 and walkTime < 177 and Input.Interact.WasPressed then
            caught = true
            this.GameObject.Scale = Unity.Vector3(0.5, 0.5, 0.5)
        end
    elseif route == "short" and at(100) then
        turn(-90)
    end

    if walkTime > 190 then
        this.GameObject.SetActive(false)
    end
end

function turn(degrees)
    local rotation = this.GameObject.LocalRotation
    rotation.y = rotation.y + degrees
    this.GameObject.LocalRotation = rotation
end

function interact()
    this.LogGraphContribution(3, 0)
end