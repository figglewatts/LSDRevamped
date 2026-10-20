audio = GetEntity(this.GameObject.Name .. "Audio").DreamAudio
videoClip = GetEntity("FlyingVideoClip").VideoClip
player = GetEntity("__player")

interacted = false
moveSpeed = 0.05
creepTime = 0
closeBy = false
held = false
holding = false

function start()
    local huge = this.GameObject.Name == "Ghost3"
    if not huge and Random.OneIn(3) then
        this.GameObject.Scale = Unity.Vector3(4, 4, 4)
    end

    this.SetChildVisible(false)
    
    if IsDayEven() or (huge and not IsDayLinear(3, 1)) then
        this.GameObject.SetActive(false)
        return
    end
end

function intervalUpdate()
    if not interacted then return end

    local playerDistance = (player.WorldPosition - this.GameObject.WorldPosition).length()
    if not closeBy and playerDistance < 0.5 then
        closeBy = true
        if Random.OneIn(2) then
            audio.Play()
            if Random.OneIn(2) then
                DreamSystem.StretchDream(0, 3)
            end
            this.Action
                .WaitUntil(Condition.WaitForSeconds(3.5))
                .Then(endDream)
                .ThenFinish()
        end
    end

    this.SnapToFloor(0.3)
end

function update()
    if not interacted then return end

    creepTime = creepTime + Unity.DeltaTime()
    if creepTime > 5 then
        this.LookAtPlane(player.WorldPosition)
    end
    this.MoveInDirection(this.Forward, moveSpeed)

    -- holds you in place for a while if you're near
    if not held and creepTime > 15 then
        held = true
        local playerDistance = (player.WorldPosition - this.GameObject.WorldPosition).length()
        if playerDistance < 2 then
            holding = true
            SetCanControlPlayer(false)
        end
    elseif holding and creepTime > 25 then
        holding = false
        SetCanControlPlayer(true)
    end
end

function interact()
    if interacted then return end
    interacted = true
    this.SetChildVisible(true)
    audio.Play()
    this.LogGraphContribution(-4, -5)
end

-- half the time the dream ends with the film, otherwise it just ends
function endDream()
    if Random.OneIn(2) then
        videoClip.Play(Unity.ColorRGB(1, 0, 0))
    else
        DreamSystem.EndDream()
    end
end