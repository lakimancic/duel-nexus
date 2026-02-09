import type { ComponentProps } from "react";

export interface CardProps extends ComponentProps<"img"> {
  name: string;
  description: string;
  type: number;

  attack?: number;
  defense?: number;
  level?: number;

  hidden: boolean;
};
