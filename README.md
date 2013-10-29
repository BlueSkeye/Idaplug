Idaplug
=======

IDA from Hex-Rays SA is a well-known dis-assembly software extensively used in
the reverse engineering field. IDA provides an extensive API for C language plugin
development. While efficient, C can be painful and time consuming as soon as you
attempt to develop high level tasks.

This project attempts to provide :
- A framework that let you develop plugins using C#
- Visual Studio 2012 integration for ease of use

Plugins are mixed mode libraries (managed+unmanaged) relying on a .Net library
wrapping the native API.

Using a wrapping layer, performances are expected to be poorer than those from the
native API. Consequently this project shouldn't be considered a catch all replacement
for the existing native API.

Disclaimer : This project is neither backed nor supported by Hex-Rays
