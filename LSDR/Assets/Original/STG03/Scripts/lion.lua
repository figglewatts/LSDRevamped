player = GetEntity("__player")
roarAudio = GetEntity(this.GameObject.Name .. "RoarAudio").DreamAudio
stepAudio = GetEntity(this.GameObject.Name .. "StepAudio").DreamAudio

moveSpeed = 0.2

function start()
    this.SetChildVisible(false)

    local directionChoice = Random.IntMax(4)
    if directionChoice == 0 then
        this.LookInDirection(this.Forward.negated())
    elseif directionChoice == 1 then
        this.LookInDirection(this.Right)
    elseif directionChoice == 2 then
        this.LookInDirection(this.Right.negated())
    end
end

function update()
    if not interacted then return end

    this.MoveInDirection(this.Forward, moveSpeed)
end

function intervalUpdate()
    if not interacted then return end

    local toPlayer = player.WorldPosition - this.GameObject.WorldPosition
    local normalisedToPlayer = toPlayer.normalise()
    local distanceToPlayer = toPlayer.length()
    if distanceToPlayer < 0.7 then
        local randResult = Random.Float()
        if randResult < 0.1 then
            -- eat the player!!!
            this.LookAtPlane(player.WorldPosition)
            SetCanControlPlayer(false)
            DreamSystem.LogGraphContributionFromEntity(0, -9)
            this.GameObject.WorldPosition = player.WorldPosition - (toPlayer * 1.5)
            roarAudio.Play()
            DreamSystem.EndDream()
            interacted = false
            return
        end
        
        if randResult < 0.4 then
            -- rotate to face the player
            this.LookAtPlane(player.WorldPosition)
        end

        -- move the player backwards
        player.WorldPosition = player.WorldPosition + (toPlayer * 1.5)
    end
end

function interact()
    this.SetChildVisible(true)
    stepAudio.Play()
    this.PlayAnimation(0)
    DreamSystem.LogGraphContributionFromEntity(-3, 3)
    interacted = true
end