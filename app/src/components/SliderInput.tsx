import React, { ChangeEvent, useCallback } from "react"
import "./SliderInput.scss"
import { Box } from "./ui/Box"
import { Text } from "./ui/Text"

type SliderInputProps = {
  id: string
  name: string
  value: number
  min: number
  max: number
  step?: number
  onChange: (e: number) => void
  disabled?: boolean
  display?: (value: number) => string
}

export function SliderInput(props: SliderInputProps) {
  const set = useCallback((e: ChangeEvent<HTMLInputElement>) => props.onChange(Number(e.target.value)), [])

  function increment() {
    if (props.disabled || props.value >= props.max) return
    props.onChange(props.value + (props.step ?? 1))
  }

  function decrement() {
    if (props.disabled || props.value <= props.min) return
    props.onChange(props.value - (props.step ?? 1))
  }

  return (
    <div className="slider-input">
      <Box onClick={decrement} disabled={props.disabled || props.value <= props.min}>
        <div className="slider-input-button-content-container">
          <Text type="unset-color">-</Text>
        </div>
      </Box>
      <input
        type="range"
        name={props.name}
        id={props.id}
        min={props.min}
        max={props.max}
        step={props.step}
        value={props.value}
        onChange={set}
        disabled={props.disabled}
      />
      <Box onClick={increment} disabled={props.disabled || props.value >= props.max}>
        <div className="slider-input-button-content-container">
          <Text type="unset-color">+</Text>
        </div>
      </Box>
      <br />
      <output htmlFor={props.id}>
        <Text>{props.display ? props.display(props.value) : props.value}</Text>
      </output>
    </div>
  )
}
