moveSpeed = 0.25

function start()
    startPosition = this.GameObject.WorldPosition
end

function update()
    local delta = Unity.DeltaTime()
    local position = this.GameObject.WorldPosition
    position.x = position.x - moveSpeed * delta
    this.GameObject.WorldPosition = position
end

function interact()

end